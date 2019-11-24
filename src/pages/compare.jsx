import React, { Component } from "react"
import Layout from "../components/GatsbyDefault/layout"
import SEO from "../components/GatsbyDefault/seo"
import SearchBar from "../components/Search/SearchBar"
import PlayerData from "../utilities/PlayerData"
import LodestoneCalendar from "../utilities/LodestoneCalendar"
import PlayerCard from "../components/Player/PlayerCard"
import ColorWheel from "../utilities/ColorWheel"
import { Card, Grid } from "semantic-ui-react"
import { ResponsiveContainer, XAxis, YAxis, AreaChart, Area, CartesianGrid, Tooltip } from "recharts"

export default class compare extends Component {
    state = {
        selectedPlayers: [],
        areas: [],
        defs: [],
        cards: []
    }

    constructor(props) {
        super(props)

        this.handleSearchSelection = this.handleSearchSelection.bind(this)
        this.colorWheel = new ColorWheel()
    }

    handleSearchSelection(selectedLodestoneId) {
        if (this.state.selectedPlayers.some(selectedPlayer => selectedPlayer.LodestoneId === selectedLodestoneId)) {
            return
        }

        new PlayerData()
            .GetPlayer(selectedLodestoneId)
            .then(selectedPlayer => {
                const newPlayer = {
                    LodestoneId: selectedLodestoneId,
                    Data: selectedPlayer,
                    Calendar: new LodestoneCalendar(selectedPlayer.Performances)
                }

                const color = this.colorWheel.getNext()

                const newDef =
                    <linearGradient key={`gradient${selectedLodestoneId}`} id={selectedLodestoneId} x1="0" y1="0" x2="0" y2="1">
                        <stop offset="5%" stopColor={color} stopOpacity={0.6} />
                        <stop offset="95%" stopColor={color} stopOpacity={0.4} />
                    </linearGradient>

                const newArea = <Area
                    key={`area${selectedLodestoneId}`}
                    type="monotone"
                    dataKey={selectedLodestoneId}
                    stroke={color}
                    fillOpacity={1}
                    fill={`url(#${selectedLodestoneId})`} />

                const newCard = <PlayerCard
                    key={`card${selectedLodestoneId}`}
                    LodestoneId={selectedLodestoneId}
                    PlayerName={selectedPlayer.PlayerName}
                    Server={selectedPlayer.Server}
                    Faction={selectedPlayer.Faction}
                    FactionRank={selectedPlayer.FactionRank}
                    PortraitUrl={selectedPlayer.PortraitUrl} />

                this.setState(prevState => {
                    return {
                        selectedPlayers: [...prevState.selectedPlayers, newPlayer],
                        areas: [...prevState.areas, newArea],
                        defs: [...prevState.defs, newDef],
                        cards: [...prevState.cards, newCard]
                    }
                })
            })
    }

    render() {
        const dataSetMap = new Map()

        this.state.selectedPlayers.forEach(selectedPlayer => {
            selectedPlayer.Calendar.GetMappedSet(true).forEach(dataPoint => {
                if (!dataSetMap.has(dataPoint.TallyingPeriodId)) {
                    dataSetMap.set(dataPoint.TallyingPeriodId, { WeekStart: dataPoint.WeekStart })
                }

                dataSetMap.get(dataPoint.TallyingPeriodId)[selectedPlayer.LodestoneId] = dataPoint.Score
            })
        })

        return (
            <Layout>
                <SEO title="Player Compare" />
                <Grid>
                    <Grid.Row>
                        <SearchBar handleSearchSelection={this.handleSearchSelection}></SearchBar>
                    </Grid.Row>
                    <Grid.Row>
                        <Card.Group>
                            {this.state.cards}
                        </Card.Group>
                    </Grid.Row>
                    <Grid.Row>
                        {!!this.state.selectedPlayers.length &&
                            <ResponsiveContainer width="100%" height={200}>
                                <AreaChart data={Array.from(dataSetMap.values())} margin={{ top: 10, right: 30, left: 0, bottom: 0 }}>
                                    <defs>
                                        {this.state.defs}
                                    </defs>
                                    <XAxis dataKey="WeekStart" />
                                    <YAxis />
                                    <CartesianGrid strokeDasharray="3 3" />
                                    <Tooltip
                                        formatter={(value, name) =>
                                            [value, this.state.selectedPlayers.find(player => player.LodestoneId === name).Data.PlayerName]}
                                    />
                                    {this.state.areas}
                                </AreaChart>
                            </ResponsiveContainer>}
                    </Grid.Row>
                </Grid>
            </Layout>
        )
    }
}