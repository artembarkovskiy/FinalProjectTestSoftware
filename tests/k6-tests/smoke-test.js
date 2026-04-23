import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    vus: 1, 
    duration: '30s',
    thresholds: {
        http_req_failed: ['rate==0.00'], 
        http_req_duration: ['p(95)<2000'], 
    },
};

const BASE_URL = 'http://localhost:5063';

export function setup() {
    let res = http.get(`${BASE_URL}/api/Recipes`);
    let recipes = res.json();

    let validRecipeId = (Array.isArray(recipes) && recipes.length > 0) ? recipes[0].id : null;

    return { recipeId: validRecipeId };
}

export default function (data) {
    let getAllRes = http.get(`${BASE_URL}/api/Recipes`);
    check(getAllRes, { 'get all recipes is 200': (r) => r.status === 200 });
    sleep(1);

    const newRecipe = JSON.stringify({
        name: `Test Recipe ${Math.floor(Math.random() * 1000)}`,
        difficulty: 1, 
        maxPrepTime: 30,
        ingredients: ["Salt", "Water"]
    });

    let createRes = http.post(`${BASE_URL}/api/Recipes`, newRecipe, {
        headers: { 'Content-Type': 'application/json' }
    });

    check(createRes, {
        'create recipe is 201 or 400': (r) => [201, 400].includes(r.status)
    });
    sleep(1);

    if (data.recipeId) {
        let getByIdRes = http.get(`${BASE_URL}/api/Recipes/${data.recipeId}`);
        check(getByIdRes, { 'get by id is 200': (r) => r.status === 200 });

        const updatePayload = JSON.stringify({
            name: "Updated Recipe Name",
            difficulty: 2,
            maxPrepTime: 45
        });
        let updateRes = http.put(`${BASE_URL}/api/Recipes/${data.recipeId}`, updatePayload, {
            headers: { 'Content-Type': 'application/json' }
        });
        check(updateRes, { 'update is 204 or 400': (r) => [204, 400].includes(r.status) });
    }
    sleep(1);

    let searchRes = http.get(`${BASE_URL}/api/Recipes/search?ingredient=Salt`);
    check(searchRes, { 'search is 200': (r) => r.status === 200 });
    sleep(1);
}