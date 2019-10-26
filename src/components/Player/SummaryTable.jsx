import React, { Component } from "react"
import { Table, Dropdown, Menu } from "semantic-ui-react"
import PageMenu from "../Common/PageMenu"

export default class SummaryTable extends Component {
    constructor(props) {
        super(props)

        this.handleItemsPerPage = this.handleItemsPerPage.bind(this)
        this.handlePageSelect = this.handlePageSelect.bind(this)

        const pages = Math.ceil(this.props.Performances.length / 10)

        this.state = {
            itemsPerPage: 10,
            pages: pages,
            currentPage: pages
        }
    }

    options = [
        { key: "All", value: Infinity, text: "All" },
        { key: "10", value: 10, text: "10" },
        { key: "20", value: 20, text: "20" },
        { key: "50", value: 50, text: "50" }
    ]

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

    handleItemsPerPage(e, { value }) {
        let pages = Math.max(1, Math.ceil(this.props.Performances.length / value))

        this.setState({
            itemsPerPage: value,
            pages: pages,
            currentPage: pages
        })
    }

    handlePageSelect(currentPage) {
        this.setState({ currentPage })
    }

    formatPerformanceRecord(performance) {
        return (
            <Table.Row key={performance.IndexId} className={this.getFactionStyles(performance.Faction)}>
                <Table.Cell>{this.convertGrandCompanyIdToName(performance.Faction)}</Table.Cell>
                <Table.Cell>{`${performance.WeekStart} - ${performance.WeekEnd}`}</Table.Cell>
                <Table.Cell>{performance.Rank}</Table.Cell>
                <Table.Cell>{performance.Score}</Table.Cell>
            </Table.Row>
        )
    }

    render() {
        let displaySet

        if (this.state.itemsPerPage === Infinity) {
            displaySet = this.props.Performances.map(performance => this.formatPerformanceRecord(performance))
        } else {
            const pageStart = Math.max(0, this.props.Performances.length - (this.state.itemsPerPage * (this.state.pages - this.state.currentPage + 1)))
            const pageEnd = this.props.Performances.length - (this.state.itemsPerPage * (this.state.pages - this.state.currentPage))

            displaySet = this.props.Performances
                .slice(pageStart, pageEnd)
                .map(performance => this.formatPerformanceRecord(performance))
        }

        return (
            <div>
                <Table celled compact sortable>
                    <Table.Header>
                        <Table.Row>
                            <Table.HeaderCell>Grand Company</Table.HeaderCell>
                            <Table.HeaderCell>Tallying Period</Table.HeaderCell>
                            <Table.HeaderCell>Rank</Table.HeaderCell>
                            <Table.HeaderCell>Score</Table.HeaderCell>
                        </Table.Row>
                    </Table.Header>
                    <Table.Body>
                        {displaySet}
                    </Table.Body>
                    <Table.Footer>
                        <Table.Row>
                            <Table.HeaderCell colSpan="4">
                                <Menu className="floated">
                                    <Menu.Item>Items per page:</Menu.Item>
                                    <Menu.Item>
                                        <Dropdown
                                            options={this.options}
                                            defaultValue={this.state.itemsPerPage}
                                            onChange={this.handleItemsPerPage}
                                        />
                                    </Menu.Item>
                                </Menu>
                                <PageMenu
                                    floated="right"
                                    selectedPage={this.state.currentPage}
                                    pages={this.state.pages}
                                    onSelect={this.handlePageSelect}
                                />
                            </Table.HeaderCell>
                        </Table.Row>
                    </Table.Footer>
                </Table>
            </div>
        )
    }
}