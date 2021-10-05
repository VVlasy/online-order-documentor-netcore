import React from 'react';

import 'semantic-ui-css/semantic.min.css';
import './PhotoDocumentor.css';

import { Transition, Container } from 'semantic-ui-react';

import BarcodeScanner from './components/BarcodeScanner';
import BarcodeScannerError from './components/BarcodeScannerError';
import PhotoTaker from './components/PhotoTaker/PhotoTaker';

export default class PhotoDocumentor extends React.Component {
    constructor(props) {
        super(props);
        this.cameraRef = React.createRef();
        this.state = {
            width: window.innerWidth,
            scannedBarcode: "",
            scanningBarcode: true,
            takingPicture: false
        };

        window.eventsHooked = false;
    }

    componentDidMount() {

    }

    barcodeConfirmClick(barcode) {
        this.setState({
            scannedBarcode: barcode,
            scanningBarcode: false,
        });
    }

    photoTakerSuccess() {
        this.goToStart();
    }

    goToStart() {
        this.setState({
            scannedBarcode: "",
            takingPicture: false
        });
    }

    render() {
        return (<Container {...this.props}>
            <div className="PhotoDocumentor">
                <BarcodeScannerError>
                    <Transition animation="fade" duration={250} visible={this.state.scanningBarcode} unmountOnHide={true}
                        onHide={() => this.setState({ takingPicture: true })}>
                        <BarcodeScanner onConfirm={this.barcodeConfirmClick.bind(this)} />
                    </Transition>
                </BarcodeScannerError>

                <Transition animation="fade" duration={250} visible={this.state.takingPicture} unmountOnHide={true} onHide={() => this.setState({ scanningBarcode: true })
                }>
                    <PhotoTaker
                        barcode={this.state.scannedBarcode}
                        onCancel={this.goToStart.bind(this)}
                        onSuccess={this.photoTakerSuccess.bind(this)}/>
                </Transition>
            </div>
        </Container>
        );
    }
}