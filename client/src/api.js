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

export { fetchEmissions };