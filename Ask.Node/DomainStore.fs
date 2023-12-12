namespace Ask.Node

type Observer = {
    Name: string
    Code: CodeId
}

type Broker = {
    Name: string
    Code: CodeId
}

type Query = {
    Name: string
    Code: CodeId
}

type Domain = {
    Observers: Observer list
    Brokers: Broker list
    Queries: Query list
}

type Strategy = {
    Name: string
    Code: CodeId
}

type Visualization = {
    Name: string
    Code: CodeId
}
