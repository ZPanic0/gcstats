import React from "react"

import Layout from "../components/GatsbyDefault/layout"
import SEO from "../components/GatsbyDefault/seo"

const NotFoundPage = () => (
  <Layout>
    <SEO title="404: Not found" />
    <h1>Page not found</h1>
    <p>This route does not exist or site navigation has broken.</p>
  </Layout>
)

export default NotFoundPage
