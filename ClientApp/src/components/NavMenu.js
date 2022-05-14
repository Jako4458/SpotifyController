import * as React from 'react';
import { Link } from 'react-router-dom';
import Cookies from 'universal-cookie';
import './Template.css';
//import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
//import { Link } from 'react-router-dom';

export default class NavMenu extends React.PureComponent {
    state = {
        isOpen: false,
    };

    constructor(props) {
        super(props);
        this.state = { user: null, loading: true };
    }

    componentDidMount() {
        this.getUserDataFromSessionId();
    }


    render() {
        let cookies = new Cookies();

        let ContentLoggedIn = 
            <ul>
                {/*<Link to>Logout</Link>*/}
                <a href={ `API/User/logout?session_id=${cookies.get("SessionId")}` }><li>Logout</li></a>
                <a href="/Playlists"><li>Playlists</li></a>
                <a href="/Search"><li>Search</li></a>
                <a href="/Session"><li>Sessions</li></a>
            </ul>;

       let ContentLoggedOut = 
             <ul>
               <a href={ `API/SpotifyAPI/authorize?session_id=${cookies.get("SessionId")}` }><li>Login</li></a>
               <a href="/Playlists"><li>Playlists</li></a>
               <a href="/Search"><li>Search</li></a>
               <a href="/Session"><li>Sessions</li></a>
            </ul>;

        let content = this.state.user == null ? ContentLoggedOut : ContentLoggedIn;
        return (
            <header>
                <header className="edge">
                    <a href="/"><h1>{this.props.Title}</h1></a>
                    <nav>
                        { content }
                    </nav>
                </header>
            </header>
        );
    }

    async getUserDataFromSessionId() {
        const response = await fetch(`API/User/GetUser`,
            {
                headers: {
                    'SessionId': new Cookies().get("SessionId"),
                },

            }
        );
        if (response.ok) {
            try {
                const data = await response.json();
                this.setState({ user: data, loading: false });
            } catch (e) {
                this.setState({ playlist: { name: "Error: Response is not JSON!" }, loading: false });
            }
        } else if (response.status == 404) {
            this.setState({ playlist: { name: `Error: ${response.status}: ${response.body}` }, loading: false });
        }
    }

}
