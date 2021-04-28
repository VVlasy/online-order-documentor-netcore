import React from 'react';

import './PhotoTaker.css';

import { Container, Transition } from 'semantic-ui-react';

import PhotoCreation from './PhotoCreation';
import PhotoConfirmation from './PhotoConfirmation';

export default class PhotoTaker extends React.Component {
    constructor (props) {
        super(props);

        this.state = {
            takingPicture: true,
            confirmingPicture: false,
            addingPhoto: false,
            backPressed: false,
            images: [],
            newImage: null
        }
    }

    pictureTaken (image) {
        this.setState({
            addingPhoto: false,
            confirmingPicture: true,
            images: [...this.state.images, image],
            newImage: null
        });
    }

    cancelAddPhoto () {
        this.setState({
            confirmingPicture: true,
            addingPhoto: false
        });
    }

    removePhoto (photoIndex) {
        var array = [...this.state.images];

        array.splice(photoIndex, 1);
        this.setState({
            images: array
        });
    }

    onPictureCreatorHide () {
        if (this.state.backPressed) {
            this.state.addingPhoto ? this.cancelAddPhoto() : this.props.onCancel();
        } else {
            this.pictureTaken(this.state.newImage);
        }
    }

    onPictureConfirmationHide () {
        if (this.state.addingPhoto) {
            this.setState({
                takingPicture: true,
                backPressed: false
            });
        } else {
            this.retakePicture();
        }
    }

    render () {
        const { onSuccess, onError, ...props } = this.props;

        return (
            <Container {...props}>
                <Transition animation="fade" duration={250} visible={this.state.takingPicture} unmountOnHide={true}
                    onHide={this.onPictureCreatorHide.bind(this)}>
                    <PhotoCreation barcode={this.props.barcode}
                        onPictureTaken={(image) => this.setState({ takingPicture: false, newImage: image })}
                        onCancel={() => this.setState({ takingPicture: false, backPressed: true })} />
                </Transition>

                <Transition animation="fade" duration={250}
                    visible={this.state.confirmingPicture}
                    unmountOnHide={true}
                    onHide={this.onPictureConfirmationHide.bind(this)}>
                    <PhotoConfirmation
                        images={this.state.images}
                        barcode={this.props.barcode}
                        onCancel={this.props.onCancel}
                        onSuccess={this.props.onSuccess}
                        onError={this.props.onError}
                        onPhotoRemove={this.removePhoto.bind(this)}
                        onPhotoAdd={() => this.setState({ confirmingPicture: false, addingPhoto: true })} />
                </Transition>
            </Container>
        );
    }
}
