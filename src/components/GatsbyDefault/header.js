import { Link } from "gatsby"
import React, { Component } from "react"
import { Menu, Header } from "semantic-ui-react"
import MessageBlock from "../Common/MessageBlock"

export default class SiteHeader extends Component {

  getLink({ name, location, text }) {
    return (
      <Link
        key={name}
        to={location}
        className={`item${this.props.activeItem === name ? " active" : ""}`}
      >
        {text}
      </Link>
    )
  }

  render() {
    const rightNav = [
      { name: "player", location: "/player/", text: "Player Search" },
      { name: "about", location: "/about/", text: "About" }
    ].map(navItem => this.getLink(navItem))

    return (
      <header>
        <div
          style={{
            margin: `0 auto`,
            maxWidth: 960,

          }}
        >
          <Menu pointing secondary>
            <Header
              as="h1"
              className={`item${this.props.activeItem === "home" ? " active" : ""}`}
              style={{ paddingBottom: 12, paddingLeft: 12 }}
            >
              <Link to="/">
                {this.props.siteTitle}
              </Link>
            </Header>
            <Menu.Menu position="right">
              {rightNav}
            </Menu.Menu>
          </Menu>
          <MessageBlock />
        </div>
      </header>
    )
  }
}
