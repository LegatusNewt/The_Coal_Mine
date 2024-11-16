import axios from 'axios';

axios.defaults.headers.post['Content-Type'] ='application/json;charset=utf-8';
axios.defaults.headers.post['Access-Control-Allow-Origin'] = '*';
const fetchEmissions = async () => {
    try{
        const response = await fetch('http://localhost:5096/emissions/layer', {
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
        const response = await fetch('http://localhost:5096/emissions/coverages', {
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
        const response = await axios.post('http://localhost:5096/coverages/data', coverage);
        return response; //Response here will be the coverage object?
    } catch (error) {
        console.error("Error posting coverage", error);
        return [];
    }
}

export { fetchEmissions, postCoverage, fetchCoverages };