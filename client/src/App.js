import * as React from 'react';
import * as papa from 'papaparse'
import Map, {
  Source, Layer, Popup
} from 'react-map-gl';
import { useState, useEffect, useMemo } from 'react';
import DataPoint from './data-points';
import 'mapbox-gl/dist/mapbox-gl.css';
import { fetchEmissions } from './api';

async function read_csv() {
  const csvResponse = await fetch("./data.csv");
  const csvText = await csvResponse.text();
  const parsed = papa.parse(csvText, { header: true });
  console.log(parsed.data);
  return parsed
}

function App() {
  //TODO: Display all data hardcoded
  //TODO: Set color gradient based on range of values
  //TODO: Create state toggle between Ethane (C2H6) and Methane (Ch4).
  //TODO: Create dropdown to control state toggle
  //TODO: Create buffer size state value
  //TODO: 
  const [data, setData] = useState([]);
  const [dataLayer, setDataLayer] = useState([]);
  const [showPopup, setShowPopup] = useState(false);
  const [popupInfo, setPopupInfo] = useState(null);

  useEffect(() => {
    read_csv().then(parsedData => {
      setData(parsedData.data);
    });
  }, []);

  useEffect(() => {
    fetchEmissions().then(res => {
      setDataLayer(res);
    })
  }, []);

  //Testing geojson option
  // const geojson = {
  //   type: 'FeatureCollection',
  //   features: data.map((point, index) => ({
  //     type: 'Feature',
  //     geometry: {
  //       type: 'Point',
  //       coordinates: [point.Longitude, point.Latitude]
  //     },
  //     properties: {
  //       id: index,
  //       ...point,
  //     },
  //   })),
  // };

  const initialViewState = {
    latitude: 43.0058,
    longitude: -84.2338,
    zoom: 14,
  };

  return (
    <>
      <p className="App-header">Hello World</p>
      <div style={{ width: "100vw", height: "100vh" }}>
        <Map
          initialViewState={initialViewState}
          className="map-view"
          mapboxAccessToken="pk.eyJ1Ijoia2xhbWFyY2EiLCJhIjoiY2p5a3plOTY0MDMydDNpbzNsMDQ3ZWV2cyJ9.EA8hlPf4fj0wLkT0J0ozkA"
          mapStyle="mapbox://styles/mapbox/standard-satellite"
          onRender={(event) => event.target.resize()} //why doesn't the map fit the remaining area until this is called?
          onClick={(event) => {
            console.log("CLICKED");
            const features = event.target.queryRenderedFeatures(event.point, {
              layers: ['point']
            });
            if(features && features.length === 0) {
              return;
            }
            const pointData = features[0].properties;
            //TODO: Show all point data under click target
            setPopupInfo({
              longitude: features[0].geometry.coordinates[0],
              latitude: features[0].geometry.coordinates[1],
              allData: pointData,
            })
            console.log(popupInfo);
          }}
        >
          <Source id="test-data" type="geojson" data={dataLayer}>
            <Layer
              id="point"
              type="circle"
              paint={{
                'circle-radius': 5,
                'circle-color': '#00FF00'
              }} />
          </Source>
          {popupInfo && (
          <Popup longitude={popupInfo.longitude} latitude={popupInfo.latitude}
            anchor="bottom"
            onClose={() => setPopupInfo(null)}>
            {
              <p>
                CH4: {popupInfo.allData.CH4}<br/>
                C2H6: {popupInfo.allData.C2H6}<br/>
                Latitude: {popupInfo.latitude}<br/>
                Longitude: {popupInfo.longitude}<br/>
                TimeStamp: {popupInfo.allData.TimeStamp}<br/>
              </p>
            }
          </Popup>)}
        </Map>

      </div>
    </>
  );
}

export default App;
