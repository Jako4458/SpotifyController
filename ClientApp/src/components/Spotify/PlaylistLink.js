import * as React from 'react';

import '../Template.css';
import './Style.css';


export default class PlaylistLink extends React.Component {

    //constructor(props) {
    //    super(props);
    //    //this.state = {loading: true };
    //}

    render() {
        return (
            <React.Fragment>
                <a href={"/playlist/" + this.props.id}>
                    <div className="playlistLink" id={this.props.id}>
                        <img src={this.props.imageUrl} alt="platlist cover" />
                        <p id="name">{this.props.name}</p>
                    </div>
                </a>
            </React.Fragment>
        );
    }
}