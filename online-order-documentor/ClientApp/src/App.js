import React from 'react';

import 'semantic-ui-css/semantic.min.css';
import './App.css';

import PhotoDocumentor from './PhotoDocumentor';
import Version from './components/Version';


import ErrorNotFound from './components/ErrorNotFound';

import {
    BrowserRouter as Router,
    Switch,
    Route,
    Link
} from "react-router-dom";

export default class App extends React.Component {
    constructor(props) {
        super(props);
        this.cameraRef = React.createRef();
        this.state = {
            width: window.innerWidth
        };

        window.eventsHooked = false;
    }

    forceSWupdate() {
        if ('serviceWorker' in navigator) {
            navigator.serviceWorker.getRegistrations().then(function (registrations) {
                registrations.map(r => {
                    r.unregister();
                });
            });
            window.location.reload(true);
        }
    }

    render() {
        return (
            <Router>
                <div className="App">
                    <header className="App-header">
                        <Switch>
                            <Route exact path="/">
                                <PhotoDocumentor />
                            </Route>
                            <Route path='*' exact={true}>
                                <ErrorNotFound />
                            </Route>
                        </Switch>

                        <Version />
                    </header>
                </div>
            </Router>
        );
    }
}