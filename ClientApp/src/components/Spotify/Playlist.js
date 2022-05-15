import * as React from 'react';
import Cookies from 'universal-cookie';

import '../Template.css';
import './Style.css';

import Track from './Track';


export default class Playlist extends React.Component {

    constructor(props) {
        super(props);
        this.state = { playlist: {}, loading: true };
    }

    componentDidMount() {
        this.populatePlaylistData();
    }

    render() {
        let content = this.state.playlist.tracks == null
            ? <p>Loading ...</p>
            : this.state.playlist.tracks.items.map(element => {
                let track = element.track;
                return <Track id={track.id} name={track.name} album={track.album} duration_ms={track.duration_ms} SpotifySessionId={""} />
            });

        return (
            <React.Fragment>
                <h2>{this.state.playlist.name}</h2>
                <div className="className">
                    {content}
                </div>

            </React.Fragment>
        );
    }

    RenderIfWidthIsOver(minWidth, component) {
        return (window.innerWidth > minWidth && component)
    }

    async populatePlaylistData() {
        let sessionId = new Cookies().get("SessionId");
        const response = await fetch(`API/SpotifyAPI/GetPlaylist?playlistId=${this.props.match.params.id}`,
            {
                headers: {
                    'SessionId': sessionId,
                }
            }
        );
        if (response.ok) {
            try {
                const data = await response.json();
                this.setState({ playlist: data, loading: false });
            } catch (e) {
                this.setState({ playlist: { name: "Error: Response is not JSON!" }, loading: false });
            }
        } else if (response.status == 401) {
            window.location.href = `API/SpotifyAPI/authorize?session_id=${sessionId}&redirect_uri=/playlist/${this.props.match.params.id}`
        } else {
            this.setState({ playlist: { name: `Error: ${response.status}: ${response.body}` }, loading: false });
        }
    }
}