import * as React from 'react';
import QRCode from "react-qr-code";

import '../Template.css';
import './Style.css';


export default class Session extends React.Component {

    constructor(props) {
        super(props);
        this.name = this.props.name ? this.props.name : this.props.id
    }
   
    render() {
        return (
            <React.Fragment>
                <a href={`/session/${this.props.id}/search`}>
                    <div className="playlistLink" id={this.props.id}>
                        {/*<img src={this.props.imageUrl} alt="platlist cover" />*/}
                        <QRCode value={this.props.url}/>
                        <p id="name">{this.name}</p>
                    </div>
                </a>
            </React.Fragment>
        );
    }
}