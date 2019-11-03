import React, { Component } from "react"
import { Link } from "gatsby"
import Layout from "../components/GatsbyDefault/layout"
import SEO from "../components/GatsbyDefault/seo"

export default class IndexPage extends Component {
  render() {
    return (
      <Layout>
        <SEO title="Home" />
        <Link to="/player/">Go to Player Search</Link>
      </Layout>
    )
  }
}
