import * as React from 'react';
import * as papa from 'papaparse'
import Map, {
  Source, Layer, Popup
} from 'react-map-gl';
import { useState, useEffect, useMemo } from 'react';
import DataPoint from './data-points';
import 'mapbox-gl/dist/mapbox-gl.css';

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
  const [showPopup, setShowPopup] = useState(false);
  const [popupInfo, setPopupInfo] = useState(null);

  useEffect(() => {
    read_csv().then(parsedData => {
      setData(parsedData.data);
    });
  }, []);

  //Testing geojson option
  const geojson = {
    type: 'FeatureCollection',
    features: data.map((point, index) => ({
      type: 'Feature',
      geometry: {
        type: 'Point',
        coordinates: [point.Longitude, point.Latitude]
      },
      properties: {
        id: index,
        ...point,
      },
    })),
  };

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
            setPopupInfo(null);
            console.log("CLICKED");
            const features = event.target.queryRenderedFeatures(event.point, {
              layers: ['point']
            });
            if (features.length > 0) {
              const pointData = features[0].properties;
              const json = JSON.stringify(features);
              //TODO: Show all point data under click target
              console.log(json);
              setPopupInfo({
                longitude: features[0].geometry.coordinates[0],
                latitude: features[0].geometry.coordinates[1],
                allData: json,
              })
            }
          }}
        >
          <Source id="test-data" type="geojson" data={geojson}>
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
            {`Emissions: ${popupInfo.json}`}
          </Popup>)}
        </Map>

      </div>
    </>
  );
}

export default App;
