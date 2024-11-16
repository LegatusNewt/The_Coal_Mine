import axios from 'axios';

axios.defaults.headers.post['Content-Type'] ='application/json;charset=utf-8';
axios.defaults.headers.post['Access-Control-Allow-Origin'] = '*';

const host = 'http://localhost:5000';

const fetchEmissions = async () => {
    try{
        const response = await fetch(`${host}/emissions/layer`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        const data = await response.json();
        return data;
    } catch (error) {
        console.error("Error fetching emissions layer", error);
        return [];
    }
};

const fetchCoverages = async () => {
    try{
        const response = await fetch(`${host}/coverages/layer`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        const data = await response.json();
        return data;
    } catch (error) {
        console.error("Error fetching coverages", error);
        return [];
    }
}

const postCoverage = async (coverage) => {
    try{
        const response = await axios.post(`${host}/coverages/data`, coverage);
        return response; //Response here will be the coverage object?
    } catch (error) {
        console.error("Error posting coverage", error);
        return [];
    }
}

const postBulkCoverage = async (coverage) => {
    try{
        const response = await axios.post(`${host}/coverages/data/bulk`, coverage);
        return response; //Response here will be the coverage object?
    } catch (error) {
        console.error("Error posting coverages", error);
        return [];
    }
}

export { fetchEmissions, postCoverage, fetchCoverages, postBulkCoverage };