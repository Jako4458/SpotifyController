import * as React from 'react';
//import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
//import { Link } from 'react-router-dom';
import './Template.css';

export default class NavMenu extends React.PureComponent {
    state = {
        isOpen: false,
    };

    constructor(props) {
        super(props);
        this.state.isOpen = false;
    }

    render() {
        return (
            <header>
                <header className="edge">
                    <a href="/"><h1>{this.props.Title}</h1></a>
                    <nav>
                        <ul>
                            <a href="/Playlists"><li>Playlists</li></a>
                            <a href="/Search"><li>Search</li></a>
                        </ul>
                    </nav>
                </header>
            </header>
        );
    }

    toggle = () => {
        this.setState({
            isOpen: !this.state.isOpen
        });
    }
}
