import * as React from 'react';

import '../Template.css';
import './Style.css';

import Track from './Track';

export default class Search extends React.Component {

    constructor(props) {
        super(props);
        let query = this.props.match.params.query;
        query = query ? query : "";

        this.state = {
            value: query, search_result: null, loading: true };

        this.updateInput = this.updateInput.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);

        if (query)
            this.search(query);
    }

    render() {
        let content = this.state.search_result == null && this.state.value == "" ? "" :
            this.state.search_result == null
            ? <p>Loading ...</p>
            : this.state.search_result.tracks.items.map(element => {
                let track = element;
                return <Track id={track.id} name={track.name} album={track.album} duration_ms={track.duration_ms} />
            });

        return (
            <React.Fragment>
                <div className="search">
                    <form onSubmit={this.handleSubmit}>
                        <label>
                            <input type="text" value={this.state.value} onChange={this.updateInput} />
                        </label>
                        <input type="submit" value="Search"/>
                    </form>
                </div>
                <div className="className">
                    {content}
                </div>
            </React.Fragment>
        );
    }

    async updateInput(event) {
        //alert(event.target.value);
        this.setState({ value: event.target.value });
        //this.search(this.state.value);
    }

    async handleSubmit(event) {
        event.preventDefault();
        //this.setState({ loading: true })
        this.search(this.state.value);
        //alert(this.state.value)
    }

    async search(query) {
        const response = await fetch(`API/SpotifyAPI/Search?query=${query}`)
        if (response.ok) {
            try {
                const data = await response.json();
                this.setState({ search_result: data, loading: false });
            } catch (e) {
                this.setState({ search_result: { name: "Error: Response is not JSON!" }, loading: false });
            }
        } else if (response.status == 401) {
            window.location.href = `API/SpotifyAPI/authorize?redirect_uri=/search/${this.state.value}`
        } else {
            this.setState({ search_result: {tracks:{ name: `Error: ${response.status}: ${response.body}` }}, loading: false });
        }
    }



    //async QueueTrack(trackId) {
    //    const response = await fetch(`API/SpotifyAPI/QueueTrack?trackId=${this.props.id}`,{
    //        method: "POST",
    //        headers: {
    //            'Content-Type': 'application/json'
    //        },
    //        body: JSON.stringify({"trackId": trackId}),
    //    });

    //    if (response.status == 200) {
    //        document.getElementById(`${this.props.id}`).querySelector("Button").innerText = "Queued!"
    //            document.getElementById(`${this.props.id}`).querySelector("Button").style.background = "green";
    //        setTimeout(() => {
    //            document.getElementById(`${this.props.id}`).querySelector("Button").innerText = "Queue";
    //            document.getElementById(`${this.props.id}`).querySelector("Button").style.background = "white";
    //        }, 1000);
    //    } else if (response.status == 401) {
    //        window.location.replace("/API/SpotifyAPI/Authorize");
    //    } else {
    //        console.error(`Error ${response.status} - Queue failed!`)
    //        document.getElementById(`${this.props.id}`).querySelector("Button").innerText = "Failed!"
    //        document.getElementById(`${this.props.id}`).querySelector("Button").style.background = "#C11B17";
    //        setTimeout(() => {
    //            document.getElementById(`${this.props.id}`).querySelector("Button").innerText = "Queue";
    //            document.getElementById(`${this.props.id}`).querySelector("Button").style.background = "white";
    //        }, 1000);
    //    }
    //}

    //RenderIfWidthIsOver(minWidth, component) {
    //    return (window.innerWidth > minWidth && component)
    //}
}