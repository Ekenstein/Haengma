import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import reportWebVitals from './reportWebVitals';
import { Lobby } from './Lobby'

ReactDOM.render(
  <React.StrictMode>
    <Lobby />
  </React.StrictMode>,
  document.getElementById('root')
);

reportWebVitals();
