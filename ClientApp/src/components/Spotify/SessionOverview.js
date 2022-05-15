import * as React from 'react';
import Cookies from 'universal-cookie';

import '../Template.css';
import Session from './Session';
import './Style.css';

export default class SessionOverview extends React.Component {

    constructor(props) {
        super(props);
        this.state = { sessions: {}, loading: true };
    }

    componentDidMount() {
        this.populateSessionOverview();
    }

    render() {
        let content = this.state.sessions == null
            ? <p>Loading ...</p>
            : Array.isArray(this.state.sessions)
                ? this.state.sessions.map(session => {
                    return <Session id={session.id} name={session.name} url={session.url} />
                })
                : <p>No sessions found!</p>;

        return (
            <React.Fragment>
                <div className="playlists">
                    {content}
                </div>

            </React.Fragment>
        );
    }


    async populateSessionOverview() {
        //let sessionId = new Cookies().get("SessionId");
        const response = await fetch(`API/SpotifySession/GetPublicSessions`
            //,
            //{
            //    headers: {
            //        'SessionId': sessionId,
            //    }
            //}
        );
        if (response.ok) {
            try {
                const data = await response.json();
                this.setState({ sessions: data, loading: false });
            } catch (e) {
                this.setState({ sessions: { name: "Error: Response is not JSON!" }, loading: false });
            }
        } else {
            this.setState({ sessions: { name: `Error: ${response.status}: ${response.body}` }, loading: false });
        }
    }
}