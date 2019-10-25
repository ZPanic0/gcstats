export default class QueryStringDictionary {
    constructor(queryString) {
        this.dictionary = new Map()

        const queries = queryString.substring(1).split("&")
        queries.forEach(query => {
            let [key, value] = query.split("=")

            if (key && value) {
                this.dictionary.set(key, value)
            }
        })
    }

    get(key) {
        return this.dictionary.get(key)
    }
}