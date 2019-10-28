import { Link } from "gatsby"
import React, { Component } from "react"
import { Menu, Header } from "semantic-ui-react"


export default class SiteHeader extends Component {
  render() {
    return (
      <header style={{ margin: "2rem 0 2rem 0" }}>
        <div
          style={{
            margin: `0 auto`,
            maxWidth: 960,
            padding: `1.45rem 1.0875rem`,
          }}
        >
          <Menu pointing secondary>
            <Header className={`item${this.props.activeItem === "home" ? " active" : ""}`}>
              <Link to="/">
                {this.props.siteTitle}
              </Link>
            </Header>
            <Menu.Menu position="right">
              <Link to="/player/" className={`item${this.props.activeItem === "player" ? " active" : ""}`}>
                Player Search
                </Link>
            </Menu.Menu>
          </Menu>
        </div>
      </header>
    )
  }
}
