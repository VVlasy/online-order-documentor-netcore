import React from 'react';

export default class BarcodeScannerError extends React.Component {
    constructor (props) {
        super(props);
        this.state = {
            errorFound: false
        };
    }

    componentDidCatch (error, info) {
        this.setState({
            errorFound: true
        });
        console.log('error: ', error);
        console.log('info: ', info);
    }

    render () {
        if (this.state.errorFound) {
            // TODO: check if error is recoverable
            return this.props.children;
        }
        
        return this.props.children;
    }
}