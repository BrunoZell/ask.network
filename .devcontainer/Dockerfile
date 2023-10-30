FROM ubuntu:22.04

WORKDIR /home/

RUN apt-get update && \
    apt-get upgrade -y && \
    apt-get install -y \
    curl \
    wget \
    git \
    gnupg2 \
    jq \
    sudo \
    zsh \
    vim \
    build-essential \
    openssl \
    libclang-dev \
    libssl-dev \
    pkg-config \
    libudev-dev

# Install rustup and common components
RUN curl https://sh.rustup.rs -sSf | sh -s -- -y
ENV PATH="/root/.cargo/bin:${PATH}"

RUN rustup install stable && \
    rustup component add rustfmt && \
    rustup component add rustfmt --toolchain stable && \
    rustup component add clippy && \
    rustup component add clippy --toolchain stable

RUN cargo install cargo-expand && \
    cargo install cargo-edit

# build and install solana (from source)
RUN git clone https://github.com/solana-labs/solana
WORKDIR /home/solana
ENV RUST_REED_SOLOMON_ERASURE_ARCH=native
ENV RUSTFLAGS="-C target-feature=-avx2"
RUN git checkout v1.16.18
RUN ./scripts/cargo-install-all.sh .
RUN export PATH="/home/solana"/bin:"$PATH" && \
    solana-install init v1.16.18
ENV PATH="/root/.local/share/solana/install/active_release/bin:$PATH"

# install anchor via avm
RUN cargo install --git https://github.com/coral-xyz/anchor avm --locked --force

RUN avm install latest && \
    avm use latest

# install nvm
RUN mkdir -p /usr/local/nvm
ENV NVM_DIR /usr/local/nvm
# IMPORTANT: set the exact version
ENV NODE_VERSION v18.18.2
RUN curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.5/install.sh | bash
RUN /bin/bash -c "source $NVM_DIR/nvm.sh && nvm install $NODE_VERSION && nvm use --delete-prefix $NODE_VERSION"
# add node and npm to the PATH
ENV NODE_PATH $NVM_DIR/versions/node/$NODE_VERSION/bin
ENV PATH $NODE_PATH:$PATH

# install yarn
RUN npm install -g yarn

# print version
RUN node --version
RUN npm --version
RUN yarn --version
RUN solana --version
RUN anchor --version

# Installs zsh with some plugins
RUN sh -c "$(wget -O- https://github.com/deluan/zsh-in-docker/releases/download/v1.1.5/zsh-in-docker.sh)" -- \
    -p git \
    -p ssh-agent \
    -p https://github.com/zsh-users/zsh-autosuggestions \
    -p https://github.com/zsh-users/zsh-completions
