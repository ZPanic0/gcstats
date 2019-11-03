import React, { Component } from "react"
import Layout, { NavContext } from "../components/GatsbyDefault/layout"
import { Header, Divider } from "semantic-ui-react"
import SEO from "../components/GatsbyDefault/seo"

export default class About extends Component {
    static contextType = NavContext

    tools = {
        specialThankYous: [
            { href: "https://xivapi.com/", name: "XIVAPI", description: "A powerful REST API for FFXIV data." }
        ],
        frontEnd: [
            { href: "https://www.gatsbyjs.org/", name: "GatsbyJS", description: "A framework for building React apps." },
            { href: "https://reactjs.org/", name: "React", description: "A Javascript UI library by Facebook." },
            { href: "https://react.semantic-ui.com/", name: "Semantic UI React", description: "A user interface library adapted for React." },
            { href: "http://recharts.org/en-US/", name: "Recharts", description: "A powerful library for building charts." },
            { href: "https://momentjs.com/", name: "Moment.js", description: "A date and time library for javascript." },
            { href: "https://github.com/protobufjs/protobuf.js/tree/master", name: "protobuf.js", description: "An implementation of Protocol Buffers in javascript." }
        ],
        parser: [
            { href: "https://docs.microsoft.com/en-us/dotnet/core/about", name: ".Net Core", description: "A cross platform C# framework by Microsoft." },
            { href: "https://autofac.org/", name: "Autofac", description: "An Inversion of Control container." },
            { href: "https://html-agility-pack.net/", name: "Html Agility Pack", description: "An HTML parser." },
            { href: "https://github.com/jbogard/MediatR", name: "MediatR", description: "An async mediator library." },
            { href: "https://www.newtonsoft.com/json", name: "Json.NET", description: "A .Net JSON framework." },
            { href: "https://github.com/protobuf-net/protobuf-net", name: "protobuf-net", description: "A Protocol Buffers serializer in .Net." },
            { href: "https://github.com/Taritsyn/WebMarkupMin", name: "Web Markup Minifier", description: "An HTML minifier." }
        ],
        miscellaneous: [
            { href: "https://na.finalfantasyxiv.com/lodestone/", name: "The Lodestone", description: "The official community hub for Final Fantasy XIV and from where the information on this site is derived." },
            { href: "https://git-scm.com/", name: "Git", description: "A version control system." },
            { href: "https://code.visualstudio.com/", name: "Visual Studio Code", description: "A cross platform code editor by Microsoft." },
            { href: "https://visualstudio.microsoft.com/vs/", name: "Visual Studio", description: "A full IDE by Microsoft." }
        ]
    }

    renderSection(items, sectionName) {
        return (
            <>
                <Header as="h3">
                    {sectionName}
                </Header>
                <Divider />
                <pre className="default-text-family">
                    {items.map(link => <><a href={link.href}>{link.name}</a> - {`${link.description}\n`}</>)}
                </pre>
            </>
        )
    }

    componentDidMount() {
        this.context.callback("about")
    }

    render() {
        return (
            <Layout>
                <SEO title="About" />
                <p>
                    This site is an attempt to expand upon the available weekly performance information
                    on <a href="https://na.finalfantasyxiv.com/lodestone/">The Lodestone</a>, as well as visualize
                    that information in a pleasing way. It was built to run entirely in the browser using data
                    sets built weekly on a low powered single board computer.
                </p>
                <p>The github repo can be found <a href="https://github.com/ZPanic0/gcstats">here</a>.</p>
                <Header as="h2">
                    Tools and Resources
                </Header>
                {this.renderSection(this.tools.specialThankYous, "Special Thank You To")}
                {this.renderSection(this.tools.frontEnd, "Site")}
                {this.renderSection(this.tools.parser, "Parser")}
                {this.renderSection(this.tools.miscellaneous, "Miscellaneous")}
            </Layout>
        )
    }
}