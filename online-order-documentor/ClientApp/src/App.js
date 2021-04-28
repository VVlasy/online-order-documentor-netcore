import React from 'react';

import 'semantic-ui-css/semantic.min.css';
import './App.css';

import { Transition } from 'semantic-ui-react';

import BarcodeScanner from './components/BarcodeScanner';
import BarcodeScannerError from './components/BarcodeScannerError';
import PhotoTaker from './components/PhotoTaker/PhotoTaker';
import { version } from '../package.json';

export default class App extends React.Component {
  constructor (props) {
    super(props);
    this.cameraRef = React.createRef();
    this.state = {
      width: window.innerWidth,
      scannedBarcode: "",
      scanningBarcode: true,
      takingPicture: false,
      showSuccess: false,
      showError: false
    };

    window.eventsHooked = false;
  }

  componentDidMount () {

  }

  barcodeConfirmClick (barcode) {
    this.setState({
      scannedBarcode: barcode,
      scanningBarcode: false,
    });
  }

  photoTakerSuccess () {
    this.goToStart();
    this.setState({ showSuccess: true });
  }
  photoTakerError () {
    this.setState({ showError: true });
  }

  goToStart () {
    this.setState({
      scannedBarcode: "",
      takingPicture: false
    });
  }

  onSuccessAnimationFinish () {
    setTimeout(() => this.setState({ showSuccess: false }), 5000);
  }

  onErrorAnimationFinish () {
    setTimeout(() => this.setState({ showError: false }), 5000);
  }

  render () {
    return (
      <div className="App">
        <header className="App-header">
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
              onSuccess={this.photoTakerSuccess.bind(this)}
              onError={this.photoTakerError.bind(this)} />
          </Transition>

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