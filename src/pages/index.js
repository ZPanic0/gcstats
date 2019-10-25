import React, { Component } from "react"
import { Link } from "gatsby"
import Layout from "../components/GatsbyDefault/layout"
import SEO from "../components/GatsbyDefault/seo"
import SearchBar from "../components/Search/SearchBar"
import PlayerPanel from "../components/Player/PlayerPanel"
import Messages from "../utilities/Messages"
import PlayerSource from "../utilities/PlayerSource"

export default class IndexPage extends Component {
  constructor(props) {
    super(props)

    this.handleSearchSelection = this.handleSearchSelection.bind(this)

    let messages = new Messages()

    messages.get("Indexes").then((message) => this.setState({ indexMessage: message }))
  }

  state = {
    indexMessage: null,
    selectedPlayer: null
  }

  handleSearchSelection(selectedLodestoneId) {
    new PlayerSource()
      .GetPlayer(selectedLodestoneId)
      .then((selectedPlayer) => this.setState({ selectedPlayer }))
  }

  render() {
    return (
      <Layout>
        <SEO title="Home" />
        {this.state.indexMessage && <SearchBar handleSearchSelection={this.handleSearchSelection} IndexMessage={this.state.indexMessage} />}
        {this.state.selectedPlayer && <PlayerPanel player={this.state.selectedPlayer} />}
        <Link to="/page-2/">Go to page 2</Link>
      </Layout>
    )
  }
}
