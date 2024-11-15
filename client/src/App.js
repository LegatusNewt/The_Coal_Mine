import * as React from 'react';
import * as papa from 'papaparse'
import ToggleButton from '@mui/material/ToggleButton';
import ToggleButtonGroup from '@mui/material/ToggleButtonGroup';
import Fab from '@mui/material/Fab';
import Map, {
  Source, Layer, Popup
} from 'react-map-gl';
import { useState, useEffect, useRef } from 'react';
import 'mapbox-gl/dist/mapbox-gl.css';
import { fetchEmissions } from './api';
import buffer from '@turf/buffer';

async function read_csv() {
  const csvResponse = await fetch("./data.csv");
  const csvText = await csvResponse.text();
  const parsed = papa.parse(csvText, { header: true });
  console.log(parsed.data);
  return parsed
}

function App() {
  //TODO: Set color gradient based on range of values
  //TODO: Create state toggle between Ethane (C2H6) and Methane (Ch4).
  //TODO: Create dropdown to control state toggle
  //TODO: Create buffer size state value
  //TODO: 
  const [dataLayer, setDataLayer] = useState([]);
  const [bufferLayer, setBufferLayer] = useState([]);
  const [popupInfo, setPopupInfo] = useState(false);
  const [cursorState, setCursorState] = useState('grab');
  const [selectedGas, setSelectedGas] = useState('C2H6');
  const [bufferSize, setBufferSize] = useState(10);
  const mapRef = useRef(null);

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
  const addBufferLayer = (size) => {
    // Create the buffered data using input in meters
    const bufferData = buffer(dataLayer, size, { units: 'meters' });
    setBufferLayer(bufferData);

    // If layer exists already just change the data
    if (mapRef.current.getLayer('buffer')) {
      mapRef.current.getSource('buffer').setData(bufferData);
    } else{
      // Otherwise create the new layer
      mapRef.current.addSource('buffer', {
        type: 'geojson',
        data: bufferData,
      });
      mapRef.current.addLayer({
        id: 'buffer',
        type: 'line',
        source: 'buffer',
        paint: {
          'line-color': '#000100',
          'line-width': 1,
        },
      });
    }


  };

  const handleLoad = (event) => {
    const map = event.target;
    mapRef.current = map;
    map.on('mousemove', (event) => {
      const features = map.queryRenderedFeatures(event.point, {
        layers: ['point']
      });
      if (features && features.length > 0) {
        setCursorState('pointer');
      } else {
        setCursorState('grab');
      }
    });

    map.on('click', (event) => {
      const features = map.queryRenderedFeatures(event.point, {
        layers: ['point']
      });
      if (features && features.length > 0) {
        const pointData = features[0].properties;
        setPopupInfo({
          key: pointData.id,
          longitude: features[0].geometry.coordinates[0],
          latitude: features[0].geometry.coordinates[1],
          allData: pointData
        });
      } else {
        setPopupInfo(null);
      }
    });
  };

  const initialViewState = {
    latitude: 43.0058,
    longitude: -84.2338,
    zoom: 14,
  };

  const dataColor = () => {
    if(selectedGas === 'C2H6') {
      return dataColorC2H6();
    }
    else if (selectedGas === 'CH4') {
      return dataColorCH4();
    }
  };

  const dataColorCH4 = () => {
    return [
      'interpolate',
      ['linear'],
      ['get', 'CH4'],
      2, '#90EE90', // Light green
      3, '#FFD700', // Yellow
      4, '#FF0000'  // Bright red
    ];
  };

  const dataColorC2H6 = () => {
    return [
      'interpolate',
      ['linear'],
      ['get', 'C2H6'],
      0, '#ADD8E6', // Light blue
      200, '#00BFFF', // Deep sky blue
      400, '#FF1493'  // Deep pink
    ];
  };

  const handleSelectedGas = (event, newSelectedGas) => {
    setSelectedGas(newSelectedGas);
  };
  
  return (
    <>
      <div className="App">
        <div className="App-header">
          <p className="App-title">Hello World</p>
          <div>
            <input type="text" min="1" max="100" value={bufferSize} onChange={(e) => setBufferSize(e.target.value)} />
            <Fab variant="extended" onClick={() => addBufferLayer(bufferSize)}/>
          </div>
          <div class="emission-toggle">
            <p>SELECT EMISSION</p>
            <ToggleButtonGroup
              value={selectedGas}
              exclusive
              onChange={handleSelectedGas}
              aria-label="Select Emission"
            >
              <ToggleButton value="C2H6">C2H6</ToggleButton>
              <ToggleButton value="CH4">CH4</ToggleButton>
            </ToggleButtonGroup>
          </div>
        </div>
        <div class="map-container">
          <Map

            initialViewState={initialViewState}
            className="map-view"
            mapboxAccessToken="pk.eyJ1Ijoia2xhbWFyY2EiLCJhIjoiY2p5a3plOTY0MDMydDNpbzNsMDQ3ZWV2cyJ9.EA8hlPf4fj0wLkT0J0ozkA"
            mapStyle="mapbox://styles/mapbox/standard-satellite"
            cursor={cursorState}
            onLoad={handleLoad}
            onRender={(event) => event.target.resize()} //why doesn't the map fit the remaining area until this is called?
          >

            <Source id="test-data" type="geojson" data={dataLayer}>
              <Layer
                id="point"
                type="circle"
                paint={{
                  'circle-radius': 5,
                  'circle-color': dataColor(),
                }}
              />
            </Source>
            {popupInfo && (
              <Popup key={popupInfo.key}
                longitude={popupInfo.longitude}
                latitude={popupInfo.latitude}
                anchor="bottom"
                onClose={() => setPopupInfo(null)}
                >
                {
                  <p>
                    CH4: {popupInfo.allData.CH4}<br />
                    C2H6: {popupInfo.allData.C2H6}<br />
                    Latitude: {popupInfo.latitude}<br />
                    Longitude: {popupInfo.longitude}<br />
                    TimeStamp: {popupInfo.allData.TimeStamp}<br />
                  </p>
                }
              </Popup>)}
          </Map>

        </div>
      </div>
    </>
  );
}

export default App;
