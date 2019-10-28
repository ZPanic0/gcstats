/**
 * Layout component that queries for data
 * with Gatsby's useStaticQuery component
 *
 * See: https://www.gatsbyjs.org/docs/use-static-query/
 */

import React, { Component } from "react"
import Header from "./header"
import "semantic-ui-css/semantic.min.css"
import "./layout.css"

const navContainer = { callback: null }

const NavContext = React.createContext(navContainer)

class Layout extends Component {
  constructor(props) {
    super(props)

    this.handleNavChange = this.handleNavChange.bind(this)

    navContainer.callback = this.handleNavChange
  }

  state = { activeItem: "home" }

  handleNavChange(name) {
    this.setState({ activeItem: name })
  }

  render() {
    return (
      <>
        <Header siteTitle={"GCStats"} activeItem={this.state.activeItem} />
        <div
          style={{
            margin: `0 auto`,
            maxWidth: 960,
            padding: `0px 1.0875rem 1.45rem`,
            paddingTop: 0,
          }}
        >
          <main>{this.props.children}</main>
          <footer>
            © {new Date().getFullYear()}, Built with
            {` `}
            <a href="https://www.gatsbyjs.org">Gatsby</a>
          </footer>
        </div>
      </>
    )
  }
}

export { Layout as default, NavContext }
