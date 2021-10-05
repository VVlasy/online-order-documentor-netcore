import React from 'react';

import './PhotoConfirmation.css';

import { Button, Container, Loader, Transition } from 'semantic-ui-react';

import axios from 'axios';

import eventBus from './../../helpers/EventBus';

import PhotoContainer from './PhotoContainer';

export default class PhotoConfirmation extends React.Component {
    constructor(props) {
        super(props);
        this.photosRef = React.createRef();

        this.state = {
            pictureUploading: false
        }
    }

    componentDidUpdate() {
        this.photosRef.current.scrollLeft = this.photosRef.current.scrollWidth;
    }

    uploadPicture() {
        var formData = new FormData();
        formData.append("name", this.props.barcode);
        this.props.images.forEach((image) => formData.append("images", this.dataURItoBlob(image)));

        this.setState({
            pictureUploading: true
        });

        axios.post('/api/uploadImage', formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        }).then((response) => {
            this.props.onSuccess();
            eventBus.dispatch("showSuccess", "");
        }).catch((err) => {
            eventBus.dispatch("showError", "");
            this.setState({
                pictureUploading: false
            });
        });
    }

    dataURItoBlob(dataURI) {
        var binary = atob(dataURI.split(',')[1]);
        var array = [];
        for (var i = 0; i < binary.length; i++) {
            array.push(binary.charCodeAt(i));
        }
        return new Blob([new Uint8Array(array)], { type: 'image/jpeg' });
    }

    render() {
        const { onCancel, onSuccess, onPhotoRemove, onPhotoAdd, ...props } = this.props;

        return (
            <Container {...props}>
                <div className='fullsize'>
                    <div className='photo-banner'>
                        <p>Naskenovaný kód:</p>
                        <p className='barcode-text'>{this.props.barcode}</p>
                        <p>{`Chcete fotk${this.props.images.length > 1 ? 'y' : 'u'} uložit?`}</p>
                    </div>


                    <div className='confirmingDiv' ref={this.photosRef}>
                        <Transition.Group
                            animation='scale'
                            duration={250}>
                            {this.props.images.map((value, index) => {
                                return <PhotoContainer
                                    className='photoContainer'
                                    key={index}
                                    image={value}
                                    disabled={this.state.pictureUploading}
                                    onRemove={() => this.props.onPhotoRemove(index)} />;
                            })}
                        </Transition.Group>


                        <div className='addPhoto'>
                            <Button circular size='massive' icon='add' onClick={this.props.onPhotoAdd} disabled={this.state.pictureUploading} />
                        </div>
                    </div>



                    {this.state.pictureUploading ?
                        <div className='loaderDiv'>
                            <Loader active inline='centered'>{`Nahrávám fotk${this.props.images.length > 1 ? 'y' : 'u'}...`}</Loader>
                        </div>
                        : null}

                    <div className={'photo-buttons'}>
                        <Button circular size='massive' icon='check' color='green'
                            onClick={this.uploadPicture.bind(this)}
                            disabled={this.state.pictureUploading || this.props.images.length < 1} />
                        <Button circular size='massive' icon='close' color='red'
                            onClick={this.props.onCancel}
                            disabled={this.state.pictureUploading} />
                    </div>
                </div>
            </Container>
        );
    }
}