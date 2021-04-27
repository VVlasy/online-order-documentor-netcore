import React from 'react';

import './PhotoCreation.css';

import { Button, Container } from 'semantic-ui-react';

import Webcam from 'react-webcam';

export default class PhotoCreation extends React.Component {
    constructor (props) {
        super(props);
        this.cameraRef = React.createRef();

        this.state = {
            pictureSaving: false,
            cameraReady: false
        };
    }

    pictureTaken () {
        this.setState({ pictureSaving: true });

        // makes sure animation is started before screenshot taking occurs
        setTimeout(() => {
            const image = this.cameraRef.current.getScreenshot();

            if (this.props.onPictureTaken) {
                this.props.onPictureTaken(image);
            }

            this.setState({ pictureSaving: false });
        }, 25);
    }

    onUserMedia(){
        this.setState({cameraReady: true});
    }

    render () {
        const { onPictureTaken, onCancel, ...props } = this.props;

        return (<Container {...props}>
            <div className='fullsize'>
                <div className='photo-banner'>
                    <p>Naskenovaný kód:</p>
                    <p>{this.props.barcode}</p>
                    <p>Pořiďte fotku...</p>
                </div>

                <div className='webcamDiv'>
                    <Webcam
                        ref={this.cameraRef}
                        forceScreenshotSourceSize={true}
                        screenshotQuality={1}
                        screenshotFormat={'image/jpeg'}
                        imageSmoothing={true}
                        onUserMedia={this.onUserMedia.bind(this)}
                        audio={false}
                        videoConstraints={{
                            facingMode: 'environment',
                            width: { ideal: 1536 },
                            height: { ideal: 1280 },
                        }} />
                </div>

                <div className={'photo-buttons'}>
                    <Button circular size='massive' icon='camera' onClick={this.pictureTaken.bind(this)} disabled={this.props.disabled || this.state.pictureSaving || !this.state.cameraReady} />

                    <Button circular size='massive' icon='arrow left' color='red' onClick={this.props.onCancel} disabled={this.props.disabled || this.state.pictureSaving} />
                </div>
            </div>
        </Container >);
    }
}