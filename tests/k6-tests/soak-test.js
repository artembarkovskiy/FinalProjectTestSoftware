import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";
import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '20s', target: 30 },
        { duration: '2m', target: 30 },
        { duration: '20s', target: 0 },
    ],
};

export default function () {
    const res = http.get('http://localhost:5063/api/recipes');
    check(res, {
        'status is 200': (r) => r.status === 200,
        'response time is < 800ms': (r) => r.timings.duration < 800,
    });
    sleep(1);
}

export function handleSummary(data) {
    return {
        "load-test-report.html": htmlReport(data),
    };
}