# encoding=utf-8

import sys
reload(sys)
sys.setdefaultencoding("utf8")


from index import db, as_dict, md5, User


def init_user():
	db.session.add(User('user1', md5('123456'), 100, 100, 0))
	db.session.add(User('user2', md5('123456'), 100, 100, 0))
	db.session.add(User('user3', md5('123456'), 100, 100, 0))
	db.session.commit()


def main():
	init_user()


if __name__ == '__main__':
	main()


