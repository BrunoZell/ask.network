FROM node:18

# This Dockerfile assumes the build context to be .., expecting folders "frontend" and "solana/target/idl"

WORKDIR /home/workspace/frontend

# Expected folder structure at this point:
# /home/workspace/
# └── frontend/ (current working directory)

# First copy package.json and yarn.lock to leverage Docker's layer caching. 
# Docker will rebuild this layer only if these files change, which means 
# it will skip reinstalling node modules unless dependencies change.
COPY frontend/package.json frontend/yarn.lock ./

# Expected folder structure after copying package.json and yarn.lock:
# /home/workspace/
# └── frontend/
#     ├── package.json
#     └── yarn.lock

# Install dependencies. This layer is cached and only re-run if the previous 
# layer (package.json and yarn.lock) changes, saving time on builds where
# dependencies have not changed.
RUN yarn install

# Copy the rest of the project files. This step is separated from the copying 
# of package.json and yarn.lock so that changes to the project files do not 
# cause Docker to reinstall all node modules.
COPY frontend/ ../frontend
COPY solana/ ../solana

# Expected folder structure after copying the rest of the project files:
# /home/workspace/
# ├── frontend/ (all files and directories from ask.network frontend)
# └── solana/ (all files and directories from solana)

# Build the application. This is done after copying all project files, ensuring 
# that any changes in your source code will trigger a new build.
RUN yarn build

# Expose the port that the application uses. This is necessary for Docker to 
# map this port to the host, making the application accessible outside the container.
EXPOSE 3000

# Start the application using yarn. This is the command that runs when the 
# Docker container starts up, kicking off your application.
CMD ["yarn", "start"]
