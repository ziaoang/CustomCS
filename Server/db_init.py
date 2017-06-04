# encoding=utf-8

import sys
reload(sys)
sys.setdefaultencoding("utf8")


from index import db, as_dict, md5, User


def init_user():
	userList = []
	for line in open('data.txt'):
		t = line.strip().split('\t')
		user = User()
		user.username      = t[0]
		user.password_hash = md5('123456')
		user.max_score     = int(t[1])
		user.is_save       = False
		userList.append(user)
	db.session.add_all(userList)
	db.session.commit()


def main():
	init_user()


if __name__ == '__main__':
	main()


