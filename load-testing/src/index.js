import http from "k6/http";
import { sleep } from "k6";
import { getRequest, postRequest } from "./generators";

export let options = {
    insecureSkipTLSVerify: true,
    vus: 10,
    duration: '10s'
};

const baseUrl = `http://localhost:5000/${__ENV.DATABASE}`;

export default () => {
    const requestBody = JSON.stringify(postRequest());
    http.post(baseUrl, requestBody, { headers: { "Content-Type": "application/json" } });

    const url = new URL(baseUrl);
    const requestParams = getRequest();
    url.searchParams.append("smart_watch_slug", requestParams.smart_watch_slug);
    url.searchParams.append("start", requestParams.start.toISOString());
    url.searchParams.append("end", requestParams.end.toISOString());

    http.get(url.toString());
    
    sleep(Math.random() * 2);
}
