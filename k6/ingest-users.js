import http from 'k6/http';
import { check, sleep } from 'k6';
import { SharedArray } from 'k6/data';

// Carrega a massa de dados
const users = new SharedArray('users data', function () {
  return JSON.parse(open('./users.json'));
});

export const options = {
  insecureSkipTLSVerify: true,  
  
  scenarios: {
    ingest_users: {
      executor: 'shared-iterations',
      vus: 1,
      iterations: users.length,
      maxDuration: '30s',
    },
  },
};

export default function () {  
  const user = users[__ITER];  
  const url = 'https://localhost:30443/api/user';  
  const payload = JSON.stringify(user);  
  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };

  const res = http.post(url, payload, params);

  check(res, {
    'status é 200 ou 201 (Created)': (r) => r.status === 200 || r.status === 201,
  });

  sleep(1); 
}