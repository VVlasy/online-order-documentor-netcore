import React from 'react';

import 'semantic-ui-css/semantic.min.css';
import './App.css';

import { Transition } from 'semantic-ui-react';

import eventBus from "./helpers/EventBus.js";
import { version } from '../package.json';
import PhotoDocumentor from './PhotoDocumentor';

export default class App extends React.Component {
    constructor(props) {
        super(props);
        this.cameraRef = React.createRef();
        this.state = {
            width: window.innerWidth,
            showSuccess: false,
            showError: false
        };

        window.eventsHooked = false;
    }

    componentDidMount() {
        eventBus.on("showError", (text) => {
            this.setState({ showError: true });
        });
        eventBus.on("showSuccess", (text) => {
            this.setState({ showSuccess: true });
        });
    }

    onSuccessAnimationFinish() {
        setTimeout(() => this.setState({ showSuccess: false }), 5000);
    }

    onErrorAnimationFinish() {
        setTimeout(() => this.setState({ showError: false }), 5000);
    }

    render() {
        return (
            <div className="App">
                <header className="App-header">
                    <PhotoDocumentor/>

                    <div className='version-text'>
                        Verze: {version}

                        <Transition visible={this.state.showSuccess}
                            onShow={this.onSuccessAnimationFinish.bind(this)}
                            duration={250}
                            animation='slide up'>
                            <div className='notification successNotification'>
                                <p className='notificationText'>Fotka byla nahrána</p>
                            </div>
                        </Transition>

                        <Transition visible={this.state.showError}
                            onShow={this.onErrorAnimationFinish.bind(this)}
                            duration={250}
                            animation='slide up'>
                            <div className='notification errorNotification'>
                                <p className='notificationText'>Chyba při nahrávání fotky</p>
                            </div>
                        </Transition>
                    </div>
                </header>
            </div>
        );
    }
}