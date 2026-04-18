import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";
import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '10s', target: 50 },
        { duration: '20s', target: 50 },
        { duration: '10s', target: 200 },
        { duration: '20s', target: 200 },
        { duration: '10s', target: 0 },
    ],
};

const searchTerms = ['Chicken', 'Salt', 'Tomato', 'Beef', 'Sugar', 'Water', 'Oil', 'Pork', 'Flour', 'Cheese'];

export default function () {
    const randomIngredient = searchTerms[Math.floor(Math.random() * searchTerms.length)];

    const res = http.get(`http://localhost:5063/api/recipes/search?ingredient=${randomIngredient}`);

    check(res, {
        'status is 200': (r) => r.status === 200,
        'response time is < 2000ms': (r) => r.timings.duration < 2000,
    });

    sleep(1);
}

export function handleSummary(data) {
    return {
        "load-test-report.html": htmlReport(data),
    };
}