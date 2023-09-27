# Use an official Python runtime as the base image
FROM python:3.8-slim

# Set the working directory in the container
WORKDIR /app

# Install required packages
RUN pip install requests BeautifulSoup4

# Copy the current directory contents into the container at /app
COPY . /app

# Run your script when the container launches
CMD ["python", "main.py"]