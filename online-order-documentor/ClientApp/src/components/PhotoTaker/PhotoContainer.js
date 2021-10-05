import React from 'react';

import './PhotoContainer.css';


import { Button, Container } from 'semantic-ui-react';

export default class PhotoContainer extends React.Component {

    render () {
        const { onRemove, ...props } = this.props;

        return (
            <Container {...props}>
                <img src={this.props.image} alt='camera'></img>
                <div className='removePhoto'>
                    <Button circular icon='close' color='grey' onClick={this.props.onRemove} disabled={this.props.disabled} />
                </div>
            </Container>
        )
    }
}