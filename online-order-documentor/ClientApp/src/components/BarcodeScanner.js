import React from 'react';

import './BarcodeScanner.css';

import { Button, Container, Grid } from 'semantic-ui-react';

import Quagga from '@ericblade/quagga2';

export default class BarcodeScanner extends React.Component {
    constructor(props) {
        super(props);
        this.barcodeVideoRef = React.createRef();
        this.barcodeRef = React.createRef();

        this.noBarcodeValue = '--------------';

        this.state = {
            scannedBarcode: this.noBarcodeValue
        };
    }

    orientationChanged() {
        this.refreshCamera();
    }

    refreshCamera() {
        Quagga.stop();

        this.setState({
            scannedBarcode: this.noBarcodeValue
        });

        setTimeout(() => this.startQuagga(), 100);
    }

    componentDidMount() {
        window.addEventListener('orientationchange', this.orientationChanged.bind(this));
        document.addEventListener('keyup', this.logKey.bind(this));


        this.startQuagga();

        Quagga.onDetected(this._onDetected);
        Quagga.onProcessed(this._onProcessed);

        //this.saveVideoObj();
    }

    logKey(e) {
        if (e.isComposing || e.keyCode === 229) {
            return;
        }

        if (e.key === "Enter") {
            if (this.state.scannedBarcode != this.noBarcodeValue) {
                this.props.onConfirm(this.state.scannedBarcode)
            }
            

            return;
        }

        const isLetter = (e.key >= "a" && e.key <= "z") || (e.key >= "A" && e.key <= "Z");
        const isNumber = (e.key >= "0" && e.key <= "9");
        if (e.key.length === 1 && isLetter || isNumber) {
            this.setState({
                scannedBarcode: this.state.scannedBarcode == this.noBarcodeValue ? e.key : this.state.scannedBarcode += e.key
            });
        }        
    }

    saveVideoObj() {
        if (this.barcodeVideoRef?.current.srcObject) {
            console.log('Found object');
            window.cachedVideoObject = this.barcodeVideoRef.current.srcObject;
        }
        else {
            console.log('Not found yet, rerunning...');
            setTimeout(() => this.saveVideoObj(), 300);
        }
    }

    startQuagga() {
        Quagga.init(
            {
                inputStream: {
                    name: 'Live',
                    type: 'LiveStream',
                    constraints: {
                        width: 480,
                        height: 480,
                        aspectRatio: { ideal: 1 },
                        facingMode: 'environment'
                    },
                    target: this.barcodeRef.current
                },
                decoder: {
                    readers: ['code_128_reader'],
                    multiple: false
                },
                locator: {
                    patchSize: 'medium',
                    halfSample: true
                },
                locate: true
            },
            function (err) {
                if (err) {
                    console.log(err);
                    return;
                }
                Quagga.start();
            }
        );
    }

    componentWillUnmount() {
        try {
            window.removeEventListener('orientationchange', this.orientationChanged);
        }
        catch (e) {
            console.log('Orientation change hook was already unmounted...')
        }

        try {
            Quagga.offDetected();
            Quagga.offProcessed();
        } catch (e) {
            console.log('Quagga hooks were already unmounted...');
        }

        Quagga.stop();
    }

    quaggaButtons() {
        if (this.props.showButtons) {
            return (<div className={'quagga-buttons'}>
                <Button className='docsIcon margin-top-1em disabled' circular icon='info' onClick={() => window.location.href = '/api-docs'} />
                <Button className='refreshButton' circular icon='refresh' onClick={this.refreshCamera.bind(this)} />
            </div>);
        }        
    }

    render() {
        const { onConfirm, showButtons, ...props } = this.props;

        return (<Container {...props}>
            <div className='fullsize'>
                {this.quaggaButtons()}

                <Grid className={'photo-banner'}>
                    <Grid.Row columns={1}>
                        <Grid.Column>
                            <p className='scanTitle'>Namiřte kameru na čárový kód a potvrďte tlačítkem</p>
                        </Grid.Column>
                    </Grid.Row>
                    <Grid.Row>
                        <Grid.Column columns={1}>
                            <p className='barcode-text'>{this.state.scannedBarcode}</p>
                        </Grid.Column>
                    </Grid.Row>
                </Grid>
                <div id="barcodeVideo" className="viewport" ref={this.barcodeRef}>
                    <div>
                        <video ref={this.barcodeVideoRef} src=""></video>
                        <canvas className="drawingBuffer"></canvas>
                    </div>
                </div>


                <div className='barcodeScanner-buttonbox'>
                    <Button size='massive' circular icon='check' color='green' disabled={this.state.scannedBarcode === this.noBarcodeValue}
                        onClick={() => this.props.onConfirm(this.state.scannedBarcode)} />
                </div>
            </div>
        </Container>);
    }

    getMostOccuring(arr) {
        return arr.sort((a, b) =>
            arr.filter(v => v === a).length
            - arr.filter(v => v === b).length
        ).pop();
    }

    _getMedian(arr: number[]): number {
        arr.sort((a, b) => a - b);
        const half = Math.floor(arr.length / 2);
        if (arr.length % 2 === 1) // Odd length
            return arr[half];
        return (arr[half - 1] + arr[half]) / 2.0;
    }

    _onDetected = (result) => {
        const errors: number[] = result.codeResult.decodedCodes
            .filter(_ => _.error !== undefined)
            .map(_ => _.error);
        const median = this._getMedian(errors);
        if (median < 0.10) {
            this.setState({ scannedBarcode: result.codeResult.code });
        }
        else
            ;
    }

    _onProcessed = (result) => {
        var drawingCtx = Quagga.canvas.ctx.overlay,
            drawingCanvas = Quagga.canvas.dom.overlay;

        if (result) {
            if (result.boxes) {
                drawingCtx.clearRect(0, 0, parseInt(drawingCanvas.getAttribute("width")), parseInt(drawingCanvas.getAttribute("height")));
                result.boxes.filter(function (box) {
                    return box !== result.box;
                }).forEach(function (box) {
                    Quagga.ImageDebug.drawPath(box, { x: 0, y: 1 }, drawingCtx, { color: "green", lineWidth: 2 });
                });
            }

            if (result.box) {
                // transform box
                const line = result.line;
                const fakeBox = result.box;

                const resheight = fakeBox[0][1] - fakeBox[1][1];
                //const reswidth = fakeBox[0][0] - fakeBox[2][0];

                const resultBox = [
                    [line[0].x, line[0].y + resheight / 2],
                    [line[1].x, line[1].y + resheight / 2],
                    [line[1].x, line[1].y + -1 * resheight / 2],
                    [line[0].x, line[0].y + -1 * resheight / 2]
                ];
                Quagga.ImageDebug.drawPath(resultBox, { x: 0, y: 1 }, drawingCtx, { color: "green", lineWidth: 2 });
            }

            if (result.codeResult && result.codeResult.code) {
                Quagga.ImageDebug.drawPath(result.line, { x: 'x', y: 'y' }, drawingCtx, { color: 'red', lineWidth: 3 });
            }
        }
    }
}