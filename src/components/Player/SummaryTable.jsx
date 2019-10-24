import React, { Component } from "react"
import { Table } from 'semantic-ui-react'

export default class SummaryTable extends Component {
    convertGrandCompanyIdToName(id) {
        switch (id) {
            case 1:
                return "Maelstrom"
            case 2:
                return "Order of the Twin Adder"
            case 3:
                return "Immortal Flames"
            case 0:
            default:
                throw new Error("No or invalid Grand Company Id provided.")
        }
    }

    getFactionStyles(faction) {
        switch (faction) {
            case 1:
                return "maelstrom-gradient"
            case 2:
                return "adder-gradient"
            case 3:
                return "flames-gradient"
            default:
                return ""
        }
    }

    render() {
        return (
            <Table celled>
                <Table.Header>
                    <Table.Row>
                        <Table.HeaderCell>Grand Company</Table.HeaderCell>
                        <Table.HeaderCell>Tallying Period</Table.HeaderCell>
                        <Table.HeaderCell>Rank</Table.HeaderCell>
                        <Table.HeaderCell>Score</Table.HeaderCell>
                    </Table.Row>
                </Table.Header>
                <Table.Body>
                    {this.props.Performances.map(performance =>
                        <Table.Row key={performance.IndexId} className={this.getFactionStyles(performance.Faction)}>
                            <Table.Cell>{this.convertGrandCompanyIdToName(performance.Faction)}</Table.Cell>
                            <Table.Cell>{`${performance.WeekStart} - ${performance.WeekEnd}`}</Table.Cell>
                            <Table.Cell>{performance.Rank}</Table.Cell>
                            <Table.Cell>{performance.Score}</Table.Cell>
                        </Table.Row>
                    )}
                </Table.Body>
            </Table>
        )
    }
}