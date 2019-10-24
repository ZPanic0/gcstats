import React, { Component } from "react"
import { Link } from "gatsby"
import protobuf from "protobufjs"
import Layout from "../components/GatsbyDefault/layout"
import SEO from "../components/GatsbyDefault/seo"
import SearchBar from "../components/Search/SearchBar"
import PlayerPanel from "../components/Player/PlayerPanel"

export default class IndexPage extends Component {
  constructor(props) {
    super(props)

    this.handleSearchSelection = this.handleSearchSelection.bind(this)

    protobuf.load("/all.proto", (err, root) => {
      this.setState({
        indexMessage: root.lookupType("Indexes"),
        playerMessage: root.lookupType("Players")
      })
    })
  }

  state = {
    indexMessage: null,
    playerMessage: null,
    selectedLodestoneId: null,
    selectedPlayer: null
  }

  handleSearchSelection(selectedLodestoneId) {
    this.setState({ selectedLodestoneId })

    this.fetchPlayers(selectedLodestoneId)
  }

  fetchPlayers(lodestoneId) {
    const filePath = `/players/${lodestoneId % 1000}.bin`
    fetch(filePath)
      .then((response) => {
        if (!response.ok) {
          throw new Error(`Http response returned error status while fetching ${filePath}`)
        }
        return response.arrayBuffer()
      })
      .then((buffer) => {
        const message = this.state.playerMessage.decode(new Uint8Array(buffer));
        this.setState({ selectedPlayer: message.PlayerEntries.find(player => player.LodestoneId === lodestoneId) })
        console.log(this.state.selectedPlayer)
      })
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
