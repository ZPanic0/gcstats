import React, { Component } from "react"
import { Link } from "gatsby"
import Layout from "../components/GatsbyDefault/layout"
import SEO from "../components/GatsbyDefault/seo"
import { Message } from "semantic-ui-react"

export default class IndexPage extends Component {
  render() {
    return (
      <Layout>
        <SEO title="Home" />
        <Message negative>
          <Message.Header>This site hasn't launched yet.</Message.Header>
          <p>Data here is not being updated yet. Results may be stale and bugs are probably present.</p>
        </Message>
        <Link to="/player/">Go to Player Search</Link>
      </Layout>
    )
  }
}
