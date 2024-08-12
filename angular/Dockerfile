FROM node:16.16.0
WORKDIR /app
COPY package.json .
RUN npm cache clean --force
RUN npm install
COPY . .
EXPOSE 4200 49153
CMD npm run start