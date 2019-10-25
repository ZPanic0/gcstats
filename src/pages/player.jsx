import React, { Component } from "react"
import Layout from "../components/GatsbyDefault/layout"
import SEO from "../components/GatsbyDefault/seo"
import SearchBar from "../components/Search/SearchBar"
import PlayerPanel from "../components/Player/PlayerPanel"
import PlayerData from "../utilities/PlayerData"

export default class PlayerPage extends Component {
    constructor(props) {
        super(props)
    
        this.handleSearchSelection = this.handleSearchSelection.bind(this)
      }
    
      state = {
        selectedPlayer: null
      }
    
      handleSearchSelection(selectedLodestoneId) {
        new PlayerData()
          .GetPlayer(selectedLodestoneId)
          .then((selectedPlayer) => this.setState({ selectedPlayer }))
      }
    render() {
        return (
            <Layout>
                <SEO title="Player Search" />
                <SearchBar handleSearchSelection={this.handleSearchSelection} />
                {this.state.selectedPlayer && <PlayerPanel player={this.state.selectedPlayer} />}
            </Layout>
        )
    }
}