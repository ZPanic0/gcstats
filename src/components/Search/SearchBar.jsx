import React, { Component } from "react"
import ServerSearch from "./ServerSearch"
import PlayerSearch from "./PlayerSearch"
import PlayerIndex from "../../utilities/PlayerIndex"

export default class SearchBar extends Component {
  constructor(props) {
    super(props)

    this.handleServerChange = this.handleServerChange.bind(this)
    this.handlePlayerSearchTextChange = this.handlePlayerSearchTextChange.bind(this)
    this.handlePlayerSelectionChange = this.handlePlayerSelectionChange.bind(this)
  }

  state = {
    playerIndex: [],
    playerSearchResults: [],
    selectedServer: "",
    selectedLodestoneId: "",
    playerListIsLoading: false
  }

  handleServerChange(e, { value }) {
    this.setState({ 
      selectedServer: value,
      playerListIsLoading: true
     })

    new PlayerIndex().GetIndex(value).then((indexItems) => {
      this.setState({
        playerIndex: indexItems.map((item) => {
          return {
            key: item.LodestoneId,
            value: item.LodestoneId,
            text: item.PlayerName,
            lowercasetext: item.PlayerName.toLowerCase()
          }
        }),
        playerListIsLoading: false
      })
    })
  }

  handlePlayerSearchTextChange(event, { searchQuery }) {
    const loweredQuery = searchQuery.toLowerCase()
    this.setState({
      playerSearchResults: searchQuery.length < 3
        ? []
        : this.state.playerIndex.filter(playerName => playerName.lowercasetext.includes(loweredQuery))
    })
  }

  handlePlayerSelectionChange(event, { value }) {
    this.setState({ selectedLodestoneId: value })
    this.props.handleSearchSelection(value)
  }

  render() {
    return (
      <div>
        <ServerSearch
          handleChange={this.handleServerChange}
          value={this.state.selectedServer}
        />
        <PlayerSearch
          handleChange={this.handlePlayerSelectionChange}
          handleSearchChange={this.handlePlayerSearchTextChange}
          players={this.state.playerSearchResults}
          value={this.state.selectedLodestoneId}
          loading={this.state.playerListIsLoading}
          disabled={this.state.playerListIsLoading}
        />
      </div>
    )
  }
}