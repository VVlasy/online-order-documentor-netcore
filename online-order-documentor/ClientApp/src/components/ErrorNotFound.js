import React from 'react';

import 'semantic-ui-css/semantic.min.css';
import './ErrorNotFound.css';

import axios from 'axios';

import { Transition, Loader, Container, Icon, IconGroup } from 'semantic-ui-react';


export default class ErrorNotFound extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
        };

        window.eventsHooked = false;
    }

    componentDidMount() {
    }

    render() {
        return (
            <Container className='errorNotFound'>
                <p>Stránka nenalezena!</p>

                <img src='/online-documentor-error-image.png' alt='error image' />                
            </Container>
        );
    }
}