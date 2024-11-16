import * as React from 'react';
import * as papa from 'papaparse'
import ToggleButton from '@mui/material/ToggleButton';
import ToggleButtonGroup from '@mui/material/ToggleButtonGroup';
import Fab from '@mui/material/Fab';
import Button from '@mui/material/Button';
import Map, {
  Source, Layer, Popup
} from 'react-map-gl';
import { useState, useEffect, useRef } from 'react';
import 'mapbox-gl/dist/mapbox-gl.css';
import { fetchCoverages, fetchEmissions, postCoverage } from './api';
import buffer from '@turf/buffer';
import { grey, cyan, blue } from '@mui/material/colors';
import { createTheme, ThemeProvider } from '@mui/material';

const mainTheme = createTheme({
  palette: {
    primary: {
      main: grey[500],
    },
    secondary: {
      main: cyan[500],
    },
  },
});

function App() {
  //TODO: Still getting error Style not loaded :(
  //TODO: Add button to popup to save coverage
  const [dataLayer, setDataLayer] = useState(null);
  const [bufferLayer, setBufferLayer] = useState(null);
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

  useEffect(() => {
    if (mapRef.current) {
      mapRef.current.setPaintProperty('point', 'circle-color', dataColor());
    }
  }, [selectedGas]);

  const addBufferLayer = (size) => {
    // Create the buffered data using input in meters
    const bufferData = buffer(dataLayer, size, { units: 'meters' });
    setBufferLayer(bufferData);

    // If layer exists already just change the data
    if (mapRef.current.getLayer('buffer')) {
      mapRef.current.getSource('buffer').setData(bufferData);
    } else {
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

  const saveCoverage = (Id, passedBufferSize) => {
    // Find buffer feature that matches the selected data point
    console.log(Id);
    if (bufferLayer && bufferLayer.features.length > 0) {
      const feature = bufferLayer.features.find(f => f.properties.Id === Id);
      console.log(feature);

      // Post the coverage to the server
      feature.type = "Feature";
      var bodyCoverage = {
        "Name": "Test Coverage",
        "BufferSize": parseInt(passedBufferSize),
        "Feature": JSON.stringify(feature),
      }
      postCoverage(bodyCoverage).then(res => {
        console.log(res);
      });
    }
  };

  // Describe popup component
  const popupComponent = () => {

    return <Popup key={popupInfo.key}
      longitude={popupInfo.longitude}
      latitude={popupInfo.latitude}
      anchor="bottom"
      onClose={() => setPopupInfo(null)}
    >
      {
        <div className="Popup">
          <p>
            CH4: {popupInfo.allData.CH4}<br />
            C2H6: {popupInfo.allData.C2H6}<br />
            Latitude: {popupInfo.latitude}<br />
            Longitude: {popupInfo.longitude}<br />
            TimeStamp: {popupInfo.allData.TimeStamp}<br />
          </p>
          <Button
            color='secondary'
            variant='raised'
            onClick={() => saveCoverage(popupInfo.key, bufferSize)}
            sx={{ backgroundColor: "secondary.main" }}
          >Save Coverage</Button>
        </div>
      }
    </Popup>
  }

  const handleLoad = (event) => {
    // Handle the map load event and apply the event listeners to help avoid Style Not loaded errors
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

    // Handle clicking on points to open popup info and see emissions details
    map.on('click', (event) => {
      const features = map.queryRenderedFeatures(event.point, {
        layers: ['point']
      });
      if (features && features.length > 0) {
        const pointData = features[0].properties;
        setPopupInfo({
          key: pointData.Id,
          longitude: features[0].geometry.coordinates[0],
          latitude: features[0].geometry.coordinates[1],
          allData: pointData
        });
      } else {
        setPopupInfo(null);
      }
    });

    // Add source
    map.addSource('test-data', {
      type: 'geojson',
      data: dataLayer,
    });

    // Add the data layer to the map
    map.addLayer({
      id: 'point',
      type: 'circle',
      source: 'test-data',
      paint: {
        'circle-radius': 5,
        'circle-color': dataColor(),
      }
    });
  };

  const initialViewState = {
    //TODO: Focus on the actual data points rather than hardcoded values
    latitude: 43.0058,
    longitude: -84.2338,
    zoom: 14,
  };

  const dataColor = () => {
    // Different color ranges for the two gases
    if (selectedGas === 'C2H6') {
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
      <ThemeProvider theme={mainTheme}>
        <div className="App">
          <div className="App-header">
            <p className="App-title">Hello Project Canary</p>
            <div className="App-control">
              <input type="text" min="1" max="100" value={bufferSize} onChange={(e) => setBufferSize(e.target.value)} />
              <Fab color="secondary" variant="extended" onClick={() => addBufferLayer(bufferSize)}>Add Buffer</Fab>
              <div className="emission-toggle">
                <p>SELECT EMISSION</p>
                <ToggleButtonGroup
                  value={selectedGas}
                  exclusive
                  onChange={handleSelectedGas}
                  aria-label="Select Emission"
                >
                  <ToggleButton color="secondary" value="C2H6">C2H6</ToggleButton>
                  <ToggleButton color="secondary" value="CH4">CH4</ToggleButton>
                </ToggleButtonGroup>
              </div>
            </div>
          </div>
          <div className="map-container">
            <Map

              initialViewState={initialViewState}
              className="map-view"
              mapboxAccessToken="pk.eyJ1Ijoia2xhbWFyY2EiLCJhIjoiY2p5a3plOTY0MDMydDNpbzNsMDQ3ZWV2cyJ9.EA8hlPf4fj0wLkT0J0ozkA" // public token
              mapStyle="mapbox://styles/mapbox/standard-satellite"
              cursor={cursorState}
              onLoad={event => handleLoad(event)}
              onRender={(event) => event.target.resize()} //TODO: why doesn't the map fit the remaining area until this is called?
            >
              {popupInfo && (
                popupComponent()
              )}
            </Map>

          </div>
        </div>
      </ThemeProvider>
    </>
  );
}

export default App;
