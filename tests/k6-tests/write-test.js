import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";
import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '10s', target: 20 },
        { duration: '30s', target: 20 },
        { duration: '10s', target: 0 },
    ],
};

export default function () {
    const payload = JSON.stringify({
        title: `Load Test Recipe ${Math.floor(Math.random() * 10000)}`,
        description: "Testing DB write speed",
        prepTimeMinutes: 10,
        cookTimeMinutes: 20,
        servings: 4,
        difficulty: 1,
        ingredients: [
            { name: "Flour", quantity: 500, unit: "g" },
            { name: "Water", quantity: 300, unit: "ml" }
        ],
        steps: [
            { instruction: "Mix everything together." }
        ]
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    const res = http.post('http://localhost:5063/api/recipes', payload, params);

    check(res, {
        'status is 201 Created': (r) => r.status === 201,
    });
    
    sleep(1);
}

export function handleSummary(data) {
    return {
        "write-test-report.html": htmlReport(data), 
    };
}