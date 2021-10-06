import React from 'react';

import 'semantic-ui-css/semantic.min.css';
import './Version.css';

import axios from 'axios';

import { Transition, Loader, Container, Icon, IconGroup } from 'semantic-ui-react';

import eventBus from "./../helpers/EventBus.js";
import { version } from './../../package.json';

export default class App extends React.Component {
    constructor(props) {
        super(props);
        this.cameraRef = React.createRef();
        this.state = {
            width: window.innerWidth,
            showSuccess: false,
            showError: false,
            remoteVersion: "",
            versionsMatch: false
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

        this.checkUpdates();
    }

    onSuccessAnimationFinish() {
        setTimeout(() => this.setState({ showSuccess: false }), 5000);
    }

    onErrorAnimationFinish() {
        setTimeout(() => this.setState({ showError: false }), 5000);
    }

    checkUpdates() {
        axios.get('/api/ClientApp/version').then((response) => {
            if (response.data.version === undefined) {
                console.error("Api result invalid");
                return;
            }

            this.setState({
                remoteVersion: response.data.version,
                versionsMatch: version === response.data.version
            });

            if (version !== response.data.version) {
                console.log("Versions dont match! Updating...");
                this.forceSWupdate();
            } else {
                console.log("Version is up to date");
            }
        }).catch((err) => {
            console.log("Check update failed");
        });
    }

    UpdateState(props) {
        if (this.state.remoteVersion == "") {
            return "";
        }

        if (this.state.versionsMatch) {
            return (<Icon color='green' name='check circle'/>);
        } else {
            return (<Icon color='yellow' name='warning circle' />);
        }
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
            <Container className='version-text'>
                <p>Verze: {version}</p>

                <Loader active={this.state.remoteVersion === ""} inline size='mini' />
                {this.UpdateState()}


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
                </Container>
        );
    }
}