import React, { Component } from "react"
import { Link } from "gatsby"
import Layout from "../components/GatsbyDefault/layout"
import SEO from "../components/GatsbyDefault/seo"

export default class IndexPage extends Component {
  render() {
    return (
      <Layout>
        <SEO title="Home" />
        <div><Link to="/player/">Go to Player Search</Link></div>
        <div><Link to="/page-2/">Go to page 2</Link></div>
      </Layout>
    )
  }
}
