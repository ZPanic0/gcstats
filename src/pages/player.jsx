import React, { Component } from "react"
import Layout from "../components/GatsbyDefault/layout"
import SEO from "../components/GatsbyDefault/seo"
import SearchBar from "../components/Search/SearchBar"
import PlayerPanel from "../components/Player/PlayerPanel"
import PlayerData from "../utilities/PlayerData"
import QueryStringDictionary from "../utilities/QueryStringDictionary"

export default class PlayerPage extends Component {
    constructor(props) {
        super(props)

        this.handleSearchSelection = this.handleSearchSelection.bind(this)

        this.handleQueryString()
    }

    state = {
        selectedPlayer: null
    }

    handleQueryString() {
        let lodestoneId = new QueryStringDictionary(this.props.location.search).get("id")

        if (typeof lodestoneId !== "number") {
            lodestoneId = Number(lodestoneId)
        }

        if (!lodestoneId || lodestoneId === 0) {
            return
        } else {
            this.handleSearchSelection(lodestoneId)
        }
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