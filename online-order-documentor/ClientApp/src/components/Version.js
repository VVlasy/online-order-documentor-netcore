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
            successText: "",
            showError: false,
            errorText: "",
            showWarning: false,
            warningText: "",

            remoteVersion: "",
            versionsMatch: false,
            userUpdateNotified: false
        };

        window.eventsHooked = false;
    }

    componentDidMount() {
        eventBus.on("showError", (text) => {
            this.setState({
                showError: true,
                errorText: text
            });
        });
        eventBus.on("showSuccess", (text) => {
            this.setState({
                showSuccess: true,
                successText: text
            });
        });
        eventBus.on("showWarning", (text) => {
            this.setState({
                showWarning: true,
                warningText: text
            });
        });

        this.checkUpdates(false);

        setInterval(() => this.checkUpdates(true), 60 * 1000);
    }

    onSuccessAnimationFinish() {
        setTimeout(() => this.setState({ showSuccess: false }), 5000);
    }

    onErrorAnimationFinish() {
        setTimeout(() => this.setState({ showError: false }), 5000);
    }

    onWarningAnimationFinish() {
        setTimeout(() => this.setState({ showWarning: false }), 5000);
    }

    checkUpdates(isPeriodic) {
        axios.get('/api/ClientApp/version').then((response) => {
            if (response.data.version === undefined) {
                console.error("Api result invalid");
                return;
            }

            this.setState({
                remoteVersion: response.data.version,
                versionsMatch: version === response.data.version
            });

            if (!isPeriodic) {
                if (version !== response.data.version) {
                    console.log("Versions dont match! Updating...");
                    this.forceSWupdate();
                } else {
                    console.log("Version is up to date");
                }
            } else {
                if (version !== response.data.version && !this.state.userUpdateNotified) {
                    this.setState({
                        userUpdateNotified: true
                    });

                    eventBus.dispatch("showWarning", "Nový update k dispozici! Klikněte vpravo dole na verzi pro update...");
                }
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
            return (<Icon color='green' name='check circle' />);
        } else {
            return (<Icon color='yellow' name='warning circle' />);
        }
    }

    forceSWupdate() {
        eventBus.dispatch("updateStarted", "");

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
                <p onClick={this.state.remoteVersion !== version ? () => this.forceSWupdate() : null}>Verze: {version}</p>

                <Loader active={this.state.remoteVersion === ""} inline size='mini' />
                {this.UpdateState()}


                <Transition visible={this.state.showSuccess}
                    onShow={this.onSuccessAnimationFinish.bind(this)}
                    duration={250}
                    animation='slide up'>
                    <div className='notification successNotification'>
                        <p className='notificationText'>{this.state.successText}</p>
                    </div>
                </Transition>

                <Transition visible={this.state.showError}
                    onShow={this.onErrorAnimationFinish.bind(this)}
                    duration={250}
                    animation='slide up'>
                    <div className='notification errorNotification'>
                        <p className='notificationText'>{this.state.errorText}</p>
                    </div>
                </Transition>

                <Transition visible={this.state.showWarning}
                    onShow={this.onWarningAnimationFinish.bind(this)}
                    duration={250}
                    animation='slide up'>
                    <div className='notification warningNotification'>
                        <p className='notificationText'>{this.state.warningText}</p>
                    </div>
                </Transition>
            </Container>
        );
    }
}