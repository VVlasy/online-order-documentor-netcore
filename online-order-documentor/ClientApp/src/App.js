import React from 'react';

import 'semantic-ui-css/semantic.min.css';
import './App.css';

import PhotoDocumentor from './PhotoDocumentor';
import Version from './components/Version';

import eventBus from './helpers/EventBus';

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
            width: window.innerWidth,
            updateStarted: false
        };

        window.eventsHooked = false;
    }

    componentDidMount() {
        eventBus.on('updateStarted', () => {
            this.setState({ updateStarted: true });
        });
    }

    routerRender() {
        if (this.state.updateStarted) {
            return (<div style={{ margin: 'auto' }}>Aktualizace...</div>);
        }

        return (<Switch><Route exact path="/">
                                <PhotoDocumentor />
                            </Route>
                            <Route path='*' exact={true}>
                                <ErrorNotFound />
                            </Route>
                        </Switch>);
    }

    render() {
        return (
            <Router>
                <div className="App">
                    <header className="App-header">
                        {this.routerRender()}

                        <Version />
                    </header>
                </div>
            </Router>
        );
    }
}